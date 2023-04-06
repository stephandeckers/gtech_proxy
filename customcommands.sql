/**
 * @name customcommands.sql
 * @author S.Deckers
 * @date 23-March-2023
 * @description 
 */
 
delete from G3E_CUSTOMCOMMAND where g3e_username = 'CC with transaction';
insert into G3E_CUSTOMCOMMAND ( G3E_CCNO, G3E_USERNAME, G3E_DESCRIPTION, G3E_AUTHOR, G3E_COMMENTS, G3E_LARGEBITMAP, G3E_SMALLBITMAP, G3E_TOOLTIP, G3E_STATUSBARTEXT, G3E_COMMANDCLASS, G3E_ENABLINGMASK, G3E_MODALITY, G3E_SELECTSETENABLINGMASK, G3E_MENUORDINAL, G3E_LOCALECOMMENT, G3E_INTERFACE)
values (G3E_CUSTOMCOMMAND_SEQ.Nextval, 'Modeless CC debug proxy with Transaction','Modeless CC debug proxy with Transaction','Jan Stuckens','Not For use in production',0,0,'CC with transaction', null, 4, 8388624, 0, 0, 1, null, 'Proximus.ifh.DebugCCProxy:Proximus.ifh.DebugCCProxy.Modeless.TransactionController');

delete from G3E_CUSTOMCOMMAND where g3e_username = 'DummyCC';
insert into G3E_CUSTOMCOMMAND ( G3E_CCNO, G3E_USERNAME, G3E_DESCRIPTION, G3E_AUTHOR, G3E_COMMENTS, G3E_LARGEBITMAP, G3E_SMALLBITMAP, G3E_TOOLTIP, G3E_STATUSBARTEXT, G3E_COMMANDCLASS, G3E_ENABLINGMASK, G3E_MODALITY, G3E_SELECTSETENABLINGMASK, G3E_MENUORDINAL, G3E_LOCALECOMMENT, G3E_INTERFACE)
values (G3E_CUSTOMCOMMAND_SEQ.Nextval, 'DummyCC','DummyCC','Stephan Deckers','For use in production',0,0,'DummyCC', null, 4, 8388624, 0, 0, 1, null, 'Proximus.ifh.DummyCC:Proximus.ifh.DummyCC.DummyCCController');

commit;
